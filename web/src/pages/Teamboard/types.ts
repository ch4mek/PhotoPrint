import { FromToProps, HandleGridItemClick } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { generateSetFilters } from './utils';

export enum TeamboardFiltersNames {
	USER_ID = 'userId',
	USER = 'user',
	DEPARTMENT = 'department',
	GROUP = 'group',
	PROFESSIONAL_EXPERIENCE = 'professionalExperience',
	ENGLISH_LEVEL = 'englishLevel',
	HARDSKILLS = 'hardSkills',
	SPECIALIZATION = 'specialization',
	CURRENT_LOCATION = 'currentLocation',
	POSITION = 'position',
	LOCATION = 'location',
	REGISTERED_LOCATION = 'registeredLocation',
	COMMENT = 'comment',
	RESET = 'reset',
}

export interface TeamboardOptions {
	location: Array<SelectionModalOption>;
	registeredLocation: Array<SelectionModalOption>;
	position: Array<SelectionModalOption>;
	users: string[];
	hardSkills: Array<SelectionModalOption>;
	englishLevels: Array<SelectionModalOption>;
	departments: Array<SelectionModalOption>;
	groups: string[];
}

export interface TeamboardFilterType {
	userId: string;
	user: string;
	departments: string[];
	groups: string;
	professionalExperience: FromToProps<string>;
	englishLevel: string[];
	hardSkills: string[];
	specialization: string;
	currentLocation: string;
	position: string[];
	location: string[];
	registeredLocation: string[];
	comment: string;
}

export interface TeamboardAction {
	type: string;
	payload?: string[] | string | FromToProps<string> | TeamboardFilterType;
}

export interface UseTeamboardColumnsParams {
	filters: TeamboardFilterType;
	setFilters: ReturnType<typeof generateSetFilters>;
	options: TeamboardOptions;
	initialFilters: TeamboardFilterType;
	teamboardColumnsWidths: GridColumnWidthsConfiguration;
	availablePositions: string[];
	handleUserClick?: HandleGridItemClick;
}
