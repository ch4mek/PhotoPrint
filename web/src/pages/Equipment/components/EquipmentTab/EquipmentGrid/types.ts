import { SortColumn } from 'react-data-grid';
import { Maybe } from 'yup/lib/types';

import { SortDirection } from '@/constants/sort';
import { EquipmentFilters, SetFiltersType } from '@/pages/Equipment/types';
import {
	EquipmentType,
	EquipmentLocationType,
	EquipmentAvailabilityType,
	DepartmentType,
} from '@/types/configuration';

export interface EquipmentGridInnerProps {
	filters: EquipmentFilters;
	setFilters: SetFiltersType;
	rowsPerPage: number;
	page: number;
	handleOpen: (equipmentId?: string | number) => void;
	sortColumns: Maybe<readonly SortColumn[]>;
	onSortColumnsChange: (sortColumns: SortColumn[]) => void;
	setRowsPerPage: (rowsPerPageCounter: number) => void;
	setPage: (pageCounter: number) => void;
	equipmentTypes: Array<EquipmentType>;
	equipmentLocationTypes: Array<EquipmentLocationType>;
	initialFilters: EquipmentFilters;
	equipmentAvailabilityTypes: Array<EquipmentAvailabilityType>;
	departments: Array<DepartmentType>;
	order: SortDirection;
	orderBy: string;
}
