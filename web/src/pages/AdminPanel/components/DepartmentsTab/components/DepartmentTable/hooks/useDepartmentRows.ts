import { Dispatch, SetStateAction, useEffect, useState } from 'react';

import {
	DepartmentGridRow,
	DepartmentWithGroupManagersFlag,
} from '../../../types';
import { mapDepartmentsToRows } from '../utils';

export function useDepartmentRows(
	data?: DepartmentWithGroupManagersFlag[]
): [DepartmentGridRow[], Dispatch<SetStateAction<DepartmentGridRow[]>>] {
	const [rows, setRows] = useState<DepartmentGridRow[]>([]);

	useEffect(() => {
		if (data) {
			setRows(mapDepartmentsToRows(data));
		} else {
			setRows([]);
		}
	}, [data]);

	return [rows, setRows];
}
