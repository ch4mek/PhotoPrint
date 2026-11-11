import { defaultIdColumnWidth } from '@/components/Grid/constants';
import { isGroupManager, isHeadOrGroupManager, Roles } from '@/constants/roles';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { TeamboardFilterType } from './types';

export const DEFAULT_TEAMBOARD_FILTERS: TeamboardFilterType = {
	userId: '',
	user: '',
	departments: [],
	groups: '',
	professionalExperience: {
		from: null,
		to: null,
	},
	englishLevel: [],
	hardSkills: [],
	specialization: '',
	currentLocation: '',
	position: [],
	location: [],
	registeredLocation: [],
	comment: '',
};

export const getTeamboardColumnsWidths = (
	role: Roles
): GridColumnWidthsConfiguration => ({
	id: defaultIdColumnWidth,
	user: {
		width: 200,
		maxWidth: 260,
	},
	department: isHeadOrGroupManager(role)
		? 0
		: {
				width: 159,
				fixed: true,
		  },
	group: isGroupManager(role)
		? 0
		: {
				width: 87,
				fixed: true,
		  },
	professionalExperience: {
		fixed: true,
		width: 104,
	},
	englishLevel: {
		fixed: true,
		width: 94,
	},
	hardSkills: {
		fixed: true,
		width: 108,
	},
	specialization: {
		fixed: false,
		width: 200,
	},
	currentLocation: {
		width: 150,
		fixed: true,
	},
	position: {
		width: 153,
		fixed: true,
	},
	location: {
		fixed: true,
		width: 100,
	},
	registeredLocation: {
		fixed: true,
		width: 100,
	},
	comment: {
		fixed: false,
		width: 200,
	},
});

export const TEAMBOARD_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
	},
	user: {
		key: 'fullName',
		name: 'User',
	},
	department: {
		key: 'departmentFullName',
		name: 'Department',
	},
	group: {
		key: 'resourceGroupName',
		name: 'Group',
	},
	profExp: {
		key: 'careerStartedAt',
		name: 'Prof.exp.',
	},
	english: {
		key: 'englishLevelOrderWeight',
		name: 'Eng.lvl.',
	},
	hardSkills: {
		key: 'seniorityLevelOrderWeight',
		name: 'Hard skills',
	},
	primarySpecialization: {
		key: 'primarySpecializationName',
		name: 'Specialization',
	},
	currentLocation: {
		key: 'currentLocation',
		name: 'Current location',
	},
	position: {
		key: 'positionOrderWeight',
		name: 'Position',
	},
	location: {
		key: 'location',
		name: 'Location',
	},
	registeredLocation: {
		key: 'registeredLocation',
		name: 'Reg.loc.',
	},
	comments: {
		key: 'comments',
		name: 'Comment',
	},
};

export const getTeamboardColumnsAmount = (role: Roles) =>
	Object.keys(getTeamboardColumnsWidths(role)).length;
