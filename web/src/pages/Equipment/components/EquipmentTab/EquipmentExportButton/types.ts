import { EquipmentFilters } from '../../../types';

import {
	DepartmentType,
	EquipmentAvailabilityType,
	EquipmentLocationType,
	EquipmentType,
} from '@/types/configuration';

export interface EquipmentExportButtonProps {
	filters: EquipmentFilters;
	orderBy: string;
	order: string;
	allDepartments: Array<DepartmentType>;
	allEquipmentLocations: Array<EquipmentLocationType>;
	allEquipmentTypes: Array<EquipmentType>;
	allAvailabilityTypes: Array<EquipmentAvailabilityType>;
	isAddButtonShown: boolean;
}
