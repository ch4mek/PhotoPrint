import { CertificateFormValues } from '@/forms/CertificateForm/types';
import {
	ComplexConfigurableConfigurationItem,
	RoleType,
} from '@/types/configuration';
import { PersonalDataResponse } from '@/types/personalData';

export const getCertificateFormValuesFromPersonalData = ({
	personalData,
	userId,
	departmentTypes,
	roleTypes,
}: {
	personalData: PersonalDataResponse;
	userId: number;
	departmentTypes: Array<ComplexConfigurableConfigurationItem>;
	roleTypes: Array<RoleType>;
}): CertificateFormValues => ({
	...personalData,
	id: userId,
	position: roleTypes.find(({ id }) => id === personalData.position) ?? null,
	department:
		departmentTypes.find(({ id }) => id === personalData.departmentId) ?? null,
});
