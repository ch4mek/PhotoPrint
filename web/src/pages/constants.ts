import { FromToProps } from '@/components/Grid';
import { ActivityStatus } from '@/constants/activityStatus';

export const IS_ACTIVE_DEFAULT_VALUE = [ActivityStatus.Active];

export const TRACKED_TIME_DEFAULT_VALUE: FromToProps<string> = {
	from: null,
	to: null,
};

export const GRID_FROZEN_EDIT_COLUMN_DIFF = 2;

export const COLLAPSIBLE_TABLE_GRID_TEMPLATE_COLUMNS = '1fr 1fr';
export const COLLAPSIBLE_TABLE_GRID_TEMPLATE_ROWS = '1fr';
export const NO_TRACKED_HOURS_MESSAGE = 'No hours tracked yet';

export const WIKI_PAGE_CLASS_NAME = 'wiki-page';
