// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Requirement", fileName = "Requirement")]
public class RequirementObject : ScriptableObject
{
    //A list of optional requirements needed in order for this requirement to pass
    public RequirementObject[] requirements;

    //Should the result of the requirement be inverted?
    public bool InverseResults;

    //The requirement delegate
    public Func<bool> requirement;

    //Assign a condition to our requirement
    public void SetRequirement(Func<bool> requirement) => this.requirement = requirement;

    //Returns true or false if all requirements are met
    public bool IsRequirementMet()
    {
        //If no optional requirements were passed we simply get the result of this requirement
        if (requirements.Length == 0)
        {
            return HasRequirement();
        }
        else
        {
            //Temp variable to check optional requirements
            bool passed = true;

            //Itterate over optional requirements, break the loop if any are false
            foreach (var r in requirements)
            {
                if (r.IsRequirementMet() == false)
                {
                    passed = false;
                    break;
                }
            }

            //Return the inverse value of passed if we did not pass the requirement
            if (passed == false)
                return InverseResults ? passed == false : passed == true;

            //If no requirement has been assigned we return only the passed variable
            if (requirement == null)
            {
                return InverseResults? passed == false : passed == true;
            }
            else
            {
                return HasRequirement();
            }
        }
    }

    //Check to see if the requirement delegate is not null and if the requirement has been met
    private bool HasRequirement()
    {
        if (requirement != null)
        {
            if (InverseResults)
            {
                return !requirement.Invoke();
            }
            else
            {
                return requirement.Invoke();
            }
        }
        else
        {
            return false;
        }
    }
}
