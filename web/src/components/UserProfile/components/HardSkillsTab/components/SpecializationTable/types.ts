import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Column } from 'react-data-grid';

import { SpecializationRow } from '../../types';

export interface SpecializationTableFilters {
	specializationName: string;
}

export interface SpecializationTableFilterSetters {
	specializationName: (newName: string) => void;
	set: (state: SpecializationTableFilters) => void;
	reset: VoidFunction;
}

export interface SpecializationTableProps {
	columns: Column<SpecializationRow>[];
	rows: SpecializationRow[];
	allDates: SnapshotDateConfig[];
	gridPagination?: GridPaginationProps;
}

export interface SpecializationTableContextValues {
	allDates: Array<SnapshotDateConfig>;
}

export interface SnapshotDateConfig {
	snapshotDate: string;
	relatedSnapshotId: number;
}
