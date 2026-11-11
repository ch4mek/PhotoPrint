import { Dayjs } from '@/lib/dayjs';
import { FormSpecialization, UserSpecialization } from '@/types/teamboard';

export interface UserSpecializationsFormValues {
	snapshotDate: Dayjs;
	userSpecializations: Array<FormSpecialization>;
}

export interface UserSpecializationsFormProps {
	userId: number | undefined;
	handleClose: () => void;
	snapshotId?: number;
	onSubmit: (
		values: UserSpecializationsFormValues,
		snapshotId: number | null
	) => Promise<UserSpecialization> | undefined;
	minDate?: Date;
	isEditMode: boolean;
}
