import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { fileExtensions } from '@/constants/fileExtensions';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { LocationTypeId } from '@/types/configuration';

import { AssignmentsHistory } from './components/AssignmentsHistory';
import { EquipmentTab } from './components/EquipmentTab';

export const STORAGE_OPTIONS: Array<SelectionModalOption> = [
	{
		id: LocationTypeId.Minsk,
		description: LocationTypeId.Minsk,
	},
	{
		id: LocationTypeId.Vitebsk,
		description: LocationTypeId.Vitebsk,
	},
	{
		id: LocationTypeId.Warsaw,
		description: LocationTypeId.Warsaw,
	},
	NO_VALUE_FILTER_OPTION,
];

export enum EquipmentTabs {
	EQUIPMENT,
	ASSIGNMENTS_HISTORY,
}

export const EquipmentTabsTitles = [
	{
		id: EquipmentTabs.EQUIPMENT,
		text: 'Equipment',
	},
	{
		id: EquipmentTabs.ASSIGNMENTS_HISTORY,
		text: 'Assignments history',
	},
];

export const EQUIPMENT_TABS = [
	{ id: EquipmentTabs.EQUIPMENT, Component: EquipmentTab },
	{
		id: EquipmentTabs.ASSIGNMENTS_HISTORY,
		Component: AssignmentsHistory,
	},
];

const { xlsx } = fileExtensions;
export const exportOptions: PickerButtonOption[] = [{ value: xlsx }];
