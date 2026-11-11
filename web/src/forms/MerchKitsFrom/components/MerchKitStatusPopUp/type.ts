import { MerchKitAction } from '../../types';

export interface MerchKitStatusPopUpProps {
	kitId: number | string;
	action: MerchKitAction;
	handleCloseForm: VoidFunction;
	handleCloseModal: VoidFunction;
}
