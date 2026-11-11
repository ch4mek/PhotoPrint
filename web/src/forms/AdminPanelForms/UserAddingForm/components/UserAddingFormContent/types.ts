import { OutsideCloseHandler } from '@/forms/components/FormikWithBackup/types';

export type UserAddingFormContentProps = {
	isLoading: boolean;
	hasHRModuleClaim: boolean;
	onClose: VoidFunction;
	onSubmit: (e?: React.FormEvent<HTMLFormElement>) => void;
} & OutsideCloseHandler;
