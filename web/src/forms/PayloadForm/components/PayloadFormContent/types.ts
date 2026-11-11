import { PayloadFormTabs } from '../../constants';

import { OutsideCloseHandler } from '@/forms/components/FormikWithBackup/types';
import { FormUserAutoselectOption } from '@/types/forms';

export type PayloadFormContentProps = {
	activeTab: PayloadFormTabs;
	isLoaderVisible: boolean;
	user: FormUserAutoselectOption;
	submitForm: (() => Promise<void>) & (() => Promise<any>);
	onActiveTabChange: (newTab: PayloadFormTabs) => void;
	handleClose: VoidFunction;
} & OutsideCloseHandler;
