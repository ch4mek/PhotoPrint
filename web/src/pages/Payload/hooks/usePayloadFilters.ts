import { useMemo, useState } from 'react';

import { techDepartments } from '@/constants/departments';
import { isChiefRole, isManagerRole } from '@/constants/roles';
import { SortDirection } from '@/constants/sort';
import {
	FILTERS_FIELD_NAME,
	INITIAL_SPECIALIZATION_TITLE,
} from '@/pages/Payload/contants';
import { BodyFilters } from '@/pages/Payload/types';
import { getNeedProjectFilter } from '@/pages/Payload/utils/getNeedProjectFilter';

import { PayloadFiltersProps } from './types';

export const usePayloadFilters = ({
	departmentId,
	order,
	orderBy,
	role,
	specializationTitle,
}: PayloadFiltersProps) => {
	const [idFilter, setIdFilter] = useState('');
	const [departmentFilter, setDepartmentFilter] =
		useState<string[]>(techDepartments);
	const [employeeFilter, setEmployeeFilter] = useState('');
	const [groupFilter, setGroupFilter] = useState('');
	const [commentFilter, setCommentFilter] = useState('');
	const [npFilter, setNpFilter] = useState<string[]>([]);
	const [projectFilter, setProjectFilter] = useState('');
	const [planFilter, setPlanFilter] = useState('');

	const displayDepartmentColumn = useMemo(() => isChiefRole(role), [role]);
	const displayGroupColumn = useMemo(() => isManagerRole(role), [role]);

	const needProject = getNeedProjectFilter(npFilter);

	const responseBody: BodyFilters = {
		userId: idFilter || null,
		departmentIds: displayDepartmentColumn ? departmentFilter : [departmentId],
		comment: commentFilter || null,
		userFullName: employeeFilter || null,
		resourceGroupName: groupFilter || null,
		needProject,
		isDescendingOrdering: order === SortDirection.DESC,
		projectName: projectFilter || null,
		plan: planFilter || null,
		orderingField: FILTERS_FIELD_NAME[orderBy],
		specializationName:
			specializationTitle === INITIAL_SPECIALIZATION_TITLE
				? null
				: specializationTitle,
	};

	return {
		responseBodyFilters: responseBody,
		setters: {
			setIdFilter,
			setDepartmentFilter,
			setEmployeeFilter,
			setCommentFilter,
			setNpFilter,
			setProjectFilter,
			setPlanFilter,
			setGroupFilter,
		},
		gridFilters: {
			idFilter,
			departmentFilter,
			employeeFilter,
			commentFilter,
			npFilter,
			planFilter,
			projectFilter,
			groupFilter,
		},
		displayDepartmentColumn,
		displayGroupColumn,
	};
};
