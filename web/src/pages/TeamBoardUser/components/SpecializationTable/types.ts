import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { Column } from 'react-data-grid';

import { SpecializationRow } from '../../types';

import {
	SpecializationToDelete,
	SpecializationToPrimary,
} from '@/types/teamboard';

export interface SpecializationTableFilters {
	specializationName: string;
}

export interface SpecializationTableFilterSetters {
	specializationName: (newName: string) => void;
	set: (state: SpecializationTableFilters) => void;
	reset: VoidFunction;
}

export interface SpecializationTableProps {
	onSpecializationSetPrimaryClick: (
		specialization: SpecializationToPrimary
	) => void;
	onSpecializationDeleteClick: (specialization: SpecializationToDelete) => void;
	columns: Column<SpecializationRow>[];
	rows: SpecializationRow[];
	allDates: SnapshotDateConfig[];
	gridPagination?: GridPaginationProps;
}

export interface SpecializationTableContextValues {
	allDates: Array<SnapshotDateConfig>;
	contextMenuOption: PickerButtonOption<SpecializationRow>[];
}

export interface SnapshotDateConfig {
	snapshotDate: string;
	relatedSnapshotId: number;
}
