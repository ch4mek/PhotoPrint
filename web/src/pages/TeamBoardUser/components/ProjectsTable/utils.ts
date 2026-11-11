import { ProjectRow } from '../../types';

import { GRID_FROZEN_EDIT_COLUMN_DIFF } from '@/pages/constants';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	USER_PROJECTS_TABLE_COLUMNS_WIDTHS,
	USER_PROJECTS_SECTION_TABLE_COLUMNS_WIDTHS,
	HIDE_BOTTOM_BORDER_CLASS,
} from './constants';

export const getRowKey = (row: ProjectRow) => row.id;

export const cellClass = (row: ProjectRow) =>
	row.hideBottomBorder ? HIDE_BOTTOM_BORDER_CLASS : undefined;

export const getTeamboardTableColumnWidths = (mode: TeamboardTableMode) => {
	if (mode === TeamboardTableMode.Tab) {
		return USER_PROJECTS_TABLE_COLUMNS_WIDTHS;
	}

	return USER_PROJECTS_SECTION_TABLE_COLUMNS_WIDTHS;
};

export const getEditColumnIndex = (columnWidths: Record<string, number>) =>
	Object.keys(columnWidths).filter(Boolean).length +
	GRID_FROZEN_EDIT_COLUMN_DIFF;
