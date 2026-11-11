import { MerchKitsTransactionFormValues } from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { MerchKits } from '@/pages/Merch/components/KitsTab/types';

export type MerchKitsTransactionFormContentProps =
	FormikWithBackupRenderProps<MerchKitsTransactionFormValues> & {
		kitId: string | number;
		handleClose: VoidFunction;
		kitData?: MerchKits;
		isKitLoading: boolean;
	};
