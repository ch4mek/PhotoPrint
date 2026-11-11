import { capitalizeString } from '@/helpers/capitalizeString';

export const PROJECT_FORM_NAME = 'project';

export enum ProjectFromTabId {
	PROJECT,
	USER_ACCESS,
}

export const PROJECT_FORM_TABS = {
	project: {
		id: ProjectFromTabId.PROJECT,
		text: 'Project',
	},
	userAccess: {
		id: ProjectFromTabId.USER_ACCESS,
		text: 'User access',
	},
};

export const CAPITALIZED_PROJECT_FORM_NAME =
	capitalizeString(PROJECT_FORM_NAME);

export const MIN_HEIGHT_PROJECT_FORM_POPUP = 308;
