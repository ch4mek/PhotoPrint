import {
	isChiefRole,
	isGroupManager,
	isHeadOfDepartment,
	Roles,
} from '@/constants/roles';

const countOfFilters = {
	chiefCount: 6,
	hdCount: 5,
	gmCount: 4,
	defaultCount: 3,
};

const { chiefCount, gmCount, hdCount, defaultCount } = countOfFilters;

export const getGridColumnCountByRole = (role: Roles): number => {
	switch (true) {
		case isChiefRole(role):
			return chiefCount;

		case isHeadOfDepartment(role):
			return hdCount;

		case isGroupManager(role):
			return gmCount;

		default:
			return defaultCount;
	}
};
