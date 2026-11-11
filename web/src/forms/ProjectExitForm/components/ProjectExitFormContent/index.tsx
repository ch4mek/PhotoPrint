import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import { usePopupStylesConfig } from '../../hooks/usePopupStylesConfig';
import { useProjectExitForm } from '../../hooks/useProjectExitForm';
import { getDefaultOptions, getUserOptions } from '../../utils';

import { FormWrapper } from '@/forms/styles';
import dayjs from '@/lib/dayjs';
import { useGetClosingReasons } from '@/queries/projectExits/useGetClosingReasons';
import { useProjectExitUsersLookup } from '@/queries/projectExits/useProjectExitUsersLookup';
import { useClientsLookup } from '@/queries/useClientsLookup';
import { useProjectsLookup } from '@/queries/useProjectsLookup';

import { projectExitFormFields } from './constants';
import { ProjectExitFormContentProps } from './types';

export function ProjectExitFormContent({
	title,
	isLoading,
	onClose,
	handleOutsideClose,
}: ProjectExitFormContentProps) {
	const {
		values,
		handleFieldChange,
		handleClosingReasonChange,
		handleSubmit,
		errors,
		touched,
	} = useProjectExitForm();
	const { popUpStylesConfig } = usePopupStylesConfig();

	const { data: users = [], isLoading: isUsersLoading } =
		useProjectExitUsersLookup();

	const { data: clients = [], isLoading: isClientsLoading } = useClientsLookup(
		{}
	);

	const { data: projects = [], isLoading: isProjectsLoading } =
		useProjectsLookup();
	const { data: closingReasons = [], isLoading: isClosingReasonsLoading } =
		useGetClosingReasons();

	const userOptions = useMemo(() => getUserOptions(users), [users]);
	const clientOptions = useMemo(() => getDefaultOptions(clients), [clients]);
	const projectOptions = useMemo(() => getDefaultOptions(projects), [projects]);

	const showLoader =
		isLoading ||
		isUsersLoading ||
		isClientsLoading ||
		isProjectsLoading ||
		isClosingReasonsLoading;

	return (
		<PopUp
			type="form"
			stylesConfig={popUpStylesConfig}
			controls={{
				positiveControl: { onClick: handleSubmit },
				negativeControl: { onClick: onClose },
			}}
			onShadowClick={handleOutsideClose}
			{...{ onClose, showLoader, title }}
		>
			<FormWrapper>
				<Autoselect
					{...projectExitFormFields.user}
					value={values.user}
					options={userOptions}
					onChange={handleFieldChange(projectExitFormFields.user.name)}
					isRequired
					error={!!errors.user && touched.user}
					errorText={errors.user}
				/>
				<Autoselect
					{...projectExitFormFields.client}
					value={values.client}
					options={clientOptions}
					onChange={handleFieldChange(projectExitFormFields.client.name)}
					isRequired
					error={!!errors.client && touched.client}
					errorText={errors.client}
				/>
				<Autoselect
					{...projectExitFormFields.project}
					value={values.project}
					options={projectOptions}
					onChange={handleFieldChange(projectExitFormFields.project.name)}
					isRequired
					error={!!errors.project && touched.project}
					errorText={errors.project}
				/>
				<DatePicker
					innerLabel={projectExitFormFields.closingDate.label}
					size="fullWidth"
					errorText={touched.closingDate ? errors.closingDate : ''}
					value={dayjs(values.closingDate)}
					onChange={handleFieldChange(projectExitFormFields.closingDate.name)}
					isRequired
				/>
				<Autoselect
					{...projectExitFormFields.closingReason}
					value={values.closingReason}
					options={closingReasons}
					onChange={handleClosingReasonChange}
					isRequired
					error={!!errors.closingReason && touched.closingReason}
					errorText={errors.closingReason}
					enableCustomValue
				/>
			</FormWrapper>
		</PopUp>
	);
}
