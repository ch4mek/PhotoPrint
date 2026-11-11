import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';

import { formSections } from '../../constants';
import { FormColumn } from '../../styles';

import { FormSection } from '@/components/FormSection';
import { isIntern } from '@/constants/roles';
import { graduateJobPlacementOptions } from '@/constants/user';
import { ADMIN_PANEL_USER_FORM_FIELDS } from '@/forms/AdminPanelForms/UserAddingForm/constants';
import { useUserForm } from '@/forms/AdminPanelForms/UserAddingForm/hooks/useUserForm';
import dayjs from '@/lib/dayjs';

export function ContractDetailsSection() {
	const { values, errors, touched, handleFieldChange } = useUserForm();

	const isInternPosition = isIntern(values.position?.id);

	return (
		<FormSection title={formSections.contractDetails}>
			<FormColumn>
				<DatePicker
					innerLabel={ADMIN_PANEL_USER_FORM_FIELDS.hireDate.label}
					size="fullWidth"
					errorText={touched.hireDate ? errors.hireDate : ''}
					value={dayjs(values.hireDate)}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.hireDate.name
					)}
					isRequired
				/>
				<DatePicker
					innerLabel={ADMIN_PANEL_USER_FORM_FIELDS.contractStartDate.label}
					size="fullWidth"
					errorText={touched.contractStartDate ? errors.contractStartDate : ''}
					value={dayjs(values.contractStartDate)}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.contractStartDate.name
					)}
					isRequired={!isInternPosition}
				/>
				<DatePicker
					innerLabel={ADMIN_PANEL_USER_FORM_FIELDS.contractEndDate.label}
					size="fullWidth"
					errorText={touched.contractEndDate ? errors.contractEndDate : ''}
					value={dayjs(values.contractEndDate)}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.contractEndDate.name
					)}
					isRequired={!isInternPosition}
				/>
				<Autoselect
					{...ADMIN_PANEL_USER_FORM_FIELDS.graduateJobPlacement}
					value={values.graduateJobPlacement ?? null}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.graduateJobPlacement.name
					)}
					options={graduateJobPlacementOptions}
					error={Boolean(errors.graduateJobPlacement)}
					errorText={errors.graduateJobPlacement}
				/>
			</FormColumn>
		</FormSection>
	);
}
