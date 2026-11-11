export const getDepartmentProfileInfoValue = (
	departmentFullName: string | null,
	resourceGroupName: string | null
) =>
	resourceGroupName
		? `${departmentFullName} (Group ${resourceGroupName})`
		: departmentFullName;
