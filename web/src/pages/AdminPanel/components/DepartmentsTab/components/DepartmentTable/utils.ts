import {
	DepartmentGridRow,
	DepartmentGridRowType,
	DepartmentWithGroupManagersFlag,
} from '../../types';

import { DepartmentGroup } from '@/types/department';

const mapGroupsToRows = (data?: DepartmentGroup[], departmentId?: string) =>
	data?.map<DepartmentGridRow>(({ id, name, groupManagerEmail }) => ({
		id,
		name,
		departmentId,
		showMenu: true,
		access: groupManagerEmail ?? '',
		type: DepartmentGridRowType.Group,
	}));

export const mapDepartmentsToRows = (data: DepartmentWithGroupManagersFlag[]) =>
	data.map<DepartmentGridRow>(
		({ id, headOfDepartmentId, departmentName, groups, hasGroupManagers }) => ({
			id,
			showMenu: hasGroupManagers,
			name: departmentName,
			departmentId: id,
			access: headOfDepartmentId ?? '',
			groups: mapGroupsToRows(groups, id),
			type: DepartmentGridRowType.Department,
		})
	);
