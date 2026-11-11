import { Departments } from '@/constants/departments';
import { BodyFilters } from '@/pages/Payload/types';

export const generateRequestBodyObject = (userId: number): BodyFilters => ({
	userId: userId.toString(),
	departmentIds: [...Object.values(Departments)],
	comment: null,
	userFullName: null,
	needProject: null,
	isDescendingOrdering: false,
	projectName: null,
	plan: null,
	orderingField: 'UserId',
	specializationName: null,
	resourceGroupName: null,
});
