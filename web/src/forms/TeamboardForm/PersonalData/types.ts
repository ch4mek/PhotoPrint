import { Dayjs } from '@/lib/dayjs';
import {
	DepartmentType,
	EnglishLevelType,
	RoleType,
	SimpleConfigurableConfigurationItem,
} from '@/types/configuration';
import {
	GraduateJobPlacement,
	MilitaryServiceStatus,
	UserNameValues,
} from '@/types/personalData';

export interface PersonalDataFormProps {
	handleClose: () => void;
	onSubmit: (values: UpdatePersonalData) => Promise<object | undefined>;
	id: number;
	initialValues: PersonalDataFormValues;
	hasFullAccess: boolean;
}

export type PersonalDataFormValues = UserNameValues & {
	dateOfBirth: Dayjs | null;
	position: RoleType | null;
	department: DepartmentType | null;
	group: SimpleConfigurableConfigurationItem<number> | null;
	corporateEmail: string | null;
	personalEmailSecond: string | null;
	personalEmail: string | null;
	personalPhone: string | null;
	telegram: string | null;
	englishLevel: EnglishLevelType | null;
	registeredLocation: string | null;
	militaryServiceStatus: MilitaryServiceStatus | null;
	graduateJobPlacement: GraduateJobPlacement | null;
	linkedIn: string | null;
};

export type UpdatePersonalData = PersonalDataFormValues & {
	userId: number;
};
