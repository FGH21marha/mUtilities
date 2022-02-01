// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

// Returns the closest collider in the specified direction.
public static bool ClosestColliderInDirection(Vector3 position, float radius, Vector3 dir, LayerMask mask, float margin, System.Action<RaycastHit> onCollision)
{
    //Assume the closest point is at the worlds edge if we were to try and catch all objects in the scene
    float ClosestDistance = Mathf.Infinity;
    //Store a reference to the closest collision
    RaycastHit ClosestCollider = new RaycastHit();

    //Check for collisions at a point with a given radius, store that info in hit
    RaycastHit[] hit = Physics.SphereCastAll(position, radius, dir, Mathf.Infinity, mask);

    //Return false if no colliders are detected
    if (hit.Length == 0) return false;

    //Get the closest collider in range
    foreach (RaycastHit i in hit)
    {
        float dist = Vector3.Distance(i.point, position);
        if (dist < ClosestDistance && dist < radius * 2)
        {
            ClosestDistance = Vector3.Distance(i.point, position);
            ClosestCollider = i;
        }
        else
            continue;
    }

    //Return false if no collider was found;
    if (ClosestCollider.collider == null) return false;

    //Get the dot product between a direction and hit.normal
    float dot = Vector3.Dot(ClosestCollider.normal, dir);

    //Return false if dot is less than margin.
    if (Mathf.Abs(dot) < 1f - margin) return false;

    //Trigger event with hit argument
    onCollision?.Invoke(ClosestCollider);

    return true;
}
