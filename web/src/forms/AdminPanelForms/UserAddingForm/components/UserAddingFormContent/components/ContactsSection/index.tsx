import { formSections } from '../../constants';
import { FormColumn } from '../../styles';

import { FormSection } from '@/components/FormSection';
import { ONLY_DIGITS_AND_PLUS_REG_EXP } from '@/constants/regexp';
import { isIntern } from '@/constants/roles';
import { TextField } from '@/fields/Text';
import { ADMIN_PANEL_USER_FORM_FIELDS } from '@/forms/AdminPanelForms/UserAddingForm/constants';
import { useUserForm } from '@/forms/AdminPanelForms/UserAddingForm/hooks/useUserForm';

export function ContactsSection() {
	const { values, setFieldValue, validateField, setFieldTouched } =
		useUserForm();

	const isInternPosition = isIntern(values.position?.id);

	const handlePhoneNumberChange = (fieldName: string, fieldValue: string) => {
		const filteredValue = fieldValue.replace(ONLY_DIGITS_AND_PLUS_REG_EXP, '');

		if (filteredValue === fieldValue) {
			setFieldValue(fieldName, filteredValue);
			validateField(fieldName);
			setFieldTouched(fieldName, true);
		}
	};

	return (
		<FormSection title={formSections.contacts}>
			<FormColumn>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.personalPhone}
					onChange={handlePhoneNumberChange}
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.telegram}
					onChange={setFieldValue}
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.personalEmail}
					onChange={setFieldValue}
					isRequired={isInternPosition}
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.personalEmailSecond}
					onChange={setFieldValue}
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.linkedIn}
					onChange={setFieldValue}
				/>
			</FormColumn>
		</FormSection>
	);
}
