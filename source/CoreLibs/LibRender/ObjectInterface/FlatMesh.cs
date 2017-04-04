﻿using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct FlatMeshHandle {
		internal long id;
		internal FlatMeshHandle(long id) {
			this.id = id;
		}
	}

	internal class FlatMesh {
		internal List<Vertex2D> vertices = new List<Vertex2D>();
		internal List<int> indices = new List<int>();

		internal int gl_vao_id = 0;
		internal int gl_vert_id = 0;
		internal int gl_indices_id = 0;

		internal bool uploaded = false;

		internal FlatMeshHandle handle;

		internal FlatMesh Copy() {
			FlatMesh fm = new FlatMesh();
			fm.vertices.AddRange(vertices);
			fm.indices.AddRange(indices);

			return fm;
		}
	}

	public partial class Renderer {
		internal long flat_meshes_id = 0;
		internal Dictionary<long, int> flat_meshes_translation = new Dictionary<long, int>();
		internal List<FlatMesh> flat_meshes = new List<FlatMesh>();

		internal int AssertValid(FlatMeshHandle fmh) {
			int real;
			if (!flat_meshes_translation.TryGetValue(fmh.id, out real)) {
				throw new System.ArgumentException("Invalid FlatMeshHandle, no possible translation" + fmh.id.ToString());
			}
			if (flat_meshes.Count <= real) {
				throw new System.ArgumentException("Flat Mesh Handle ID larger than array: " + fmh.id.ToString());
			}

			if (flat_meshes[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted flat mesh: " + fmh.id.ToString());
			}
			return real;
		}

		public FlatMeshHandle AddFlatMesh(Vertex2D[] mesh, int[] indices) {
			FlatMesh fm = new FlatMesh();
			fm.vertices.AddRange(mesh);
			fm.indices.AddRange(indices);

			long id = flat_meshes_id++;
			fm.handle = new FlatMeshHandle(id);

			flat_meshes_translation.Add(id, flat_meshes.Count);
			flat_meshes.Add(fm);
			return fm.handle;
		}

		public void Update(FlatMeshHandle fmh, Vertex2D[] mesh, int[] indices) {
			int id = AssertValid(fmh);

			flat_meshes[id].vertices.Clear();
			flat_meshes[id].vertices.AddRange(mesh);
			flat_meshes[id].indices.Clear();
			flat_meshes[id].indices.AddRange(indices);
			flat_meshes[id].uploaded = false;
		}

		public void Delete(FlatMeshHandle fmh) {
			int id = AssertValid(fmh);

			GLFunc.DeleteVertexArray(flat_meshes[id].gl_vao_id);
			GLFunc.DeleteBuffer(flat_meshes[id].gl_vert_id);
			GLFunc.DeleteBuffer(flat_meshes[id].gl_indices_id);

			flat_meshes_translation.Remove(fmh.id);
			flat_meshes[id] = null;
		}
	}
}