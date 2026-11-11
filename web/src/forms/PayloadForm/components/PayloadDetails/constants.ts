import { TextAreas } from './types';

const MIN_ROWS = 5;
const MAX_ROWS = 5;
export const NEED_A_PROJECT_TITLE = 'Need a project';

export const TEXT_AREAS_BASE_PROPS: TextAreas = {
	comment: {
		name: 'comment',
		label: 'Comment',
		size: 'fullWidth',
		minRows: MIN_ROWS,
		maxRows: MAX_ROWS,
	},
	plan: {
		name: 'plan',
		label: 'Plan',
		size: 'fullWidth',
		minRows: MIN_ROWS,
		maxRows: MAX_ROWS,
	},
};
