import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Column, SortColumn } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { EducationsGridColumns } from '@/types/educations';
import { GridColumnWidth } from '@/types/grid';

export interface EducationsGridRow extends GridCellDefaultProps {
	id: number;
	educationLevel: string;
	educationalInstitution: string;
	faculty: string;
	specialization: string;
	studyForm: string;
	graduationYear: number;
	comment: string;
	handleEditClick: (id?: number | string) => VoidFunction;
}

export type EducationsGridColumnWidths = Record<
	EducationsGridColumns,
	GridColumnWidth
>;

export type EducationsFilterOptions = {
	educationLevels: Array<SelectionModalOption>;
	studyForms: Array<SelectionModalOption>;
};

export type EducationsGridProps = {
	rows: Array<EducationsGridRow>;
	columns: Array<Column<EducationsGridRow>>;
	pagination?: GridPaginationProps;
	handleSortColumnChange: (newSortColumns: Array<SortColumn>) => void;
	sortColumns: Array<SortColumn>;
	handlePopupToggle: VoidFunction;
};
