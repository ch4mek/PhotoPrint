import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';

export const BillabilityStatus = {
	Billable: 'Billable',
	NonBillable: 'Non-Billable',
};

export const billabilityOptions: Array<SelectionModalOption> = [
	convertConstantToSelectionModalOption(BillabilityStatus.Billable),
	convertConstantToSelectionModalOption(BillabilityStatus.NonBillable),
];
