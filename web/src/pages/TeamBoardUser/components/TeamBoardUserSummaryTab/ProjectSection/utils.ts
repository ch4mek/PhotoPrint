import { ProjectRow } from '../../../types';

import { formatDateString } from '@/helpers/dateToISOString';
import { UserProjectLookup } from '@/types/userProjectLookup';

export const convertUserProjectLookupToRows = (
	lookup: Array<UserProjectLookup>,
	handleEditClick: (projectId?: number | string) => () => void
): Array<ProjectRow> =>
	lookup.map(
		({ userOnProjectFrom, userOnProjectTo, isActive, ...resProps }, index) => ({
			...resProps,
			isActive,
			handleEditClick,
			userOnProjectFrom: formatDateString(userOnProjectFrom, ''),
			userOnProjectTo: formatDateString(userOnProjectTo, ''),
			hideBottomBorder: index === lookup.length - 1 ? true : undefined,
			disabled: !isActive,
		})
	);

export const getProjectTableLoaderRows = (
	amount: number
): Array<ProjectRow> => {
	const rows: Array<ProjectRow> = [];

	for (let i = 1; i <= amount; i += 1) {
		rows.push({
			id: i,
			isTimeOnProjectBillable: false,
			isActive: false,
			projectId: 0,
			projectName: '',
			userOnProjectFrom: '',
			userOnProjectTo: null,
			showSkeleton: true,
			clientName: '',
		});
	}

	return rows;
};
