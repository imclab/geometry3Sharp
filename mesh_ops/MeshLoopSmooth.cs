﻿using System;
using System.Collections.Generic;

namespace g3
{
    public class MeshLoopSmooth
    {
        public DMesh3 Mesh;
        public EdgeLoop Loop;

        public double Alpha = 0.25f;
        public int Rounds = 10;

        // reproject smoothed position to new location
        public Func<Vector3d, Vector3f, int, Vector3d> ProjectF;


        Vector3d[] SmoothedPostions;

        public MeshLoopSmooth(DMesh3 mesh, EdgeLoop loop)
        {
            Mesh = mesh;
            Loop = loop;

            SmoothedPostions = new Vector3d[Loop.Vertices.Length];

            ProjectF = null;
        }


        public virtual ValidationStatus Validate()
        {
            ValidationStatus loopStatus = MeshValidation.IsEdgeLoop(Mesh, Loop);
            return loopStatus;
        }


        public virtual bool Smooth()
        {
            int NV = Loop.Vertices.Length;

            double a = MathUtil.Clamp(Alpha, 0, 1);
            double num_rounds = MathUtil.Clamp(Rounds, 0, 10000);

            for (int round = 0; round < num_rounds; ++round) {

                // compute
                for (int i = 0; i < NV; ++i) {
                    int vid = Loop.Vertices[(i + 1) % NV];
                    Vector3d prev = Mesh.GetVertex(Loop.Vertices[i]);
                    Vector3d cur = Mesh.GetVertex(vid);
                    Vector3d next = Mesh.GetVertex(Loop.Vertices[(i + 2) % NV]);

                    Vector3d centroid = (prev + next) * 0.5;
                    SmoothedPostions[i] = (1 - a) * cur + (a) * centroid;
                }

                // bake
                for (int i = 0; i < NV; ++i) {
                    int vid = Loop.Vertices[(i + 1) % NV];
                    Mesh.SetVertex(vid, SmoothedPostions[i]);
                }
            }

            return true;
        }

    }
}
