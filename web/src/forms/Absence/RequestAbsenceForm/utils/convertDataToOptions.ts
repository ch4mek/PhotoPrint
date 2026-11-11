import { AbsenceType, UserAbsenceItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

const getArrayOption = (
	templateId: number,
	name: string
): SimpleConfigurableConfigurationItem<string | number> => ({
	id: templateId,
	name,
});

export const convertDataToOptions = (data?: Array<UserAbsenceItem>) => {
	if (!data) {
		return [];
	}

	return data
		.filter(({ type }) => type !== AbsenceType.Documents)
		.map(({ name, inquiryTemplateId, agreementTemplateId }) =>
			getArrayOption(inquiryTemplateId ?? agreementTemplateId, name)
		);
};
