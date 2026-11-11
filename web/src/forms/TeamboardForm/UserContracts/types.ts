import { Dayjs } from '@/lib/dayjs';
import {
	CreateUserContract,
	UpdateUserContract,
} from '@/queries/teamboard/types';
import { PersistentConfigurationItem } from '@/types/configuration';

export interface UserContractsFormProps {
	id: number;
	handleClose: () => void;
	onCreate: (values: CreateUserContract) => Promise<object>;
	onUpdate?: (values: UpdateUserContract) => Promise<object>;
	isContractOnly?: boolean;
}

export interface UserContractsFormFields {
	startDate: Dayjs | null;
	endDate: Dayjs | null;
	fireDate: Dayjs | null;
	fireType: PersistentConfigurationItem | null;
	fireReason: string;
	fireDescription: string;
}
