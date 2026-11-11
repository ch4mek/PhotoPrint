import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';

import { BillabilityStatus } from './billabilityStatus';

export const ActivityStatus = {
	Active: 'Active',
	Inactive: 'Inactive',
	Deleted: 'Deleted',
};

export const activityLogicalOptions = [
	ActivityStatus.Active,
	ActivityStatus.Inactive,
] as [string, string];

export const activityWithDeletedOptions = [
	ActivityStatus.Active,
	ActivityStatus.Deleted,
] as [string, string];

export const billabilityLogicalOptions = [
	BillabilityStatus.Billable,
	BillabilityStatus.NonBillable,
] as [string, string];

export const ACTIVATE = 'Activate';
export const DEACTIVATE = 'Deactivate';

export const getDeactivatingPopUpTitle = (entityName: string) =>
	`${entityName} deactivating`;

export const getActivatingPopUpTitle = (entityName: string) =>
	`${entityName} activating`;

export const getArchivingPopUpTitle = (entityName: string) =>
	`${entityName} archiving`;

export const getAddingPopUpTitle = (entityName: string) =>
	`${entityName} adding`;

export const getEditingPopUpTitle = (entityName: string) =>
	`${entityName} editing`;

export const getDeletingPopUpTitle = (entityName: string) =>
	`${entityName} deleting`;

export const getEditingDeactivatedEntityMessage = (entityName: string) =>
	`You can't edit deactivated ${entityName}.`;

export const statusOptions: Array<SelectionModalOption> = [
	{
		id: ActivityStatus.Active,
		description: ActivityStatus.Active,
		disabled: false,
	},
	{
		id: ActivityStatus.Inactive,
		description: ActivityStatus.Inactive,
		disabled: false,
	},
];

export const statusWithDeletedOptions: Array<SelectionModalOption> = [
	{
		id: ActivityStatus.Active,
		description: ActivityStatus.Active,
		disabled: false,
	},
	{
		id: ActivityStatus.Deleted,
		description: ActivityStatus.Deleted,
		disabled: false,
	},
];

export const activityStatusOptions: Array<SelectionModalOption> = [
	convertConstantToSelectionModalOption(ActivityStatus.Active),
	convertConstantToSelectionModalOption(ActivityStatus.Inactive),
];
