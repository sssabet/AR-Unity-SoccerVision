using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class HomographyTransformer
{
    public static Matrix<float> ComputeHomography(Vector2[] srcPoints, Vector2[] dstPoints)
    {
        if (srcPoints.Length != 4 || dstPoints.Length != 4)
        {
            Debug.LogError("Exactly four source and destination points are required.");
            return DenseMatrix.CreateIdentity(3);
        }

        Matrix<float> A = DenseMatrix.Create(8, 9, 0);

        for (int i = 0; i < 4; i++)
        {
            float x = srcPoints[i].x;
            float y = srcPoints[i].y;
            float u = dstPoints[i].x;
            float v = dstPoints[i].y;

            A[i * 2, 0] = -x;
            A[i * 2, 1] = -y;
            A[i * 2, 2] = -1;
            A[i * 2, 6] = x * u;
            A[i * 2, 7] = y * u;
            A[i * 2, 8] = u;

            A[i * 2 + 1, 3] = -x;
            A[i * 2 + 1, 4] = -y;
            A[i * 2 + 1, 5] = -1;
            A[i * 2 + 1, 6] = x * v;
            A[i * 2 + 1, 7] = y * v;
            A[i * 2 + 1, 8] = v;
        }

        // Compute SVD of A
        var svd = A.Svd(true);

        // The solution is the last column of V corresponding to the smallest singular value
        Vector<float> h = svd.VT.Row(svd.VT.RowCount - 1).ToRowMatrix().Transpose().Column(0);

        // Reshape h into the homography matrix H (3x3)
        Matrix<float> H = DenseMatrix.OfArray(new float[,]
        {
            { h[0], h[1], h[2] },
            { h[3], h[4], h[5] },
            { h[6], h[7], h[8] }
        });

        return H;
    }

    public static Vector2 TransformPoint(Matrix<float> H, Vector2 point)
    {
        Vector<float> homogeneousPoint = Vector<float>.Build.DenseOfArray(new float[] { point.x, point.y, 1f });
        Vector<float> transformedPoint = H * homogeneousPoint;

        // Convert back to Cartesian coordinates
        if (Mathf.Abs(transformedPoint[2]) > Mathf.Epsilon)
        {
            transformedPoint /= transformedPoint[2];
        }

        return new Vector2(transformedPoint[0], transformedPoint[1]);
    }
}
