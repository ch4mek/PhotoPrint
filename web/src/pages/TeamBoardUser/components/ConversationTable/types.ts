import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Dispatch, SetStateAction } from 'react';
import { SortColumn } from 'react-data-grid';

import {
	ConversationFilter,
	ConversationFilterSetter,
	ConversationRow,
} from '../../types';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';
import { ConversationDocument } from '@/types/document';
import { TeamboardTableMode } from '@/types/teamboard';

export interface ConversationTableProps {
	mode: TeamboardTableMode;
	rows: Array<ConversationRow>;
	setRows: Dispatch<SetStateAction<Array<ConversationRow>>>;
	filter?: ConversationFilter;
	setFilter?: ConversationFilterSetter;
	sortColumns?: Array<SortColumn>;
	onSortColumnsChange?: (newSortColumns: Array<SortColumn>) => void;
	areFiltersApplied?: boolean;
	resetFilter?: () => void;
	pagination?: GridPaginationProps;
	isLoading?: boolean;
	page?: number;
	supervisorAutocompleteOptions?: Array<string>;
	shouldHideTypeColumn?: boolean;
	isIntern?: boolean;
}

export interface DocumentRow extends DynamicHeightCellDefaultProps {
	files: ConversationDocument[];
}

export interface UseConversationColumnProps {
	mode: TeamboardTableMode;
	filter?: ConversationFilter;
	filterSetter?: ConversationFilterSetter;
	areFiltersApplied?: boolean;
	resetFilter?: VoidFunction;
	supervisorAutocompleteOptions?: Array<string>;
	feedbackOptions?: Array<SelectionModalOption>;
	shouldHideTypeColumn?: boolean;
	isIntern?: boolean;
}
