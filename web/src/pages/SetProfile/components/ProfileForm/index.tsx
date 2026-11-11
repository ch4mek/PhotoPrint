import { Button } from '@mms/mms-ui-library/components/Button';
import { memo, useCallback, useMemo } from 'react';

import { SAVE_BUTTON_TEXT, FORM_NAME } from '../constants';
import { ProfileFields } from '../ProfileFields';
import { validationSchema, validationSchemaCEO } from '../schema';
import { Wrapper, AuthFormWrapper } from '../styles';
import { ProfileFormProps, ProfileFormValues } from '../types';

import { chiefRoles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';

function ProfileFormInner({ onSubmit, isSubmitting }: ProfileFormProps) {
	const userInfo = useAuth();

	const { position, avatarFileId } = userInfo;

	const schema = useMemo(
		() =>
			!chiefRoles.includes(position) ? validationSchema : validationSchemaCEO,
		[position]
	);

	const handleSubmit = useCallback(
		(values: ProfileFormValues) => {
			const request: ProfileFormValues = {
				...values,
				avatarFileId,
			};

			onSubmit(request);
		},
		[avatarFileId, onSubmit]
	);

	return (
		<Wrapper>
			<FormikWithBackup
				enableReinitialize
				initialValues={userInfo}
				validationSchema={schema}
				onSubmit={handleSubmit}
				validateOnChange
				validateOnBlur={false}
				name={FORM_NAME}
			>
				{({
					values,
					errors,
					touched,
					setFieldValue,
					setFieldTouched,
					validateField,
				}) => (
					<AuthFormWrapper>
						<ProfileFields
							{...{
								values,
								errors,
								touched,
								setFieldValue,
								setFieldTouched,
								validateField,
							}}
						/>
						<Button type="submit" disabled={isSubmitting}>
							{SAVE_BUTTON_TEXT}
						</Button>
					</AuthFormWrapper>
				)}
			</FormikWithBackup>
		</Wrapper>
	);
}

export const SetProfileForm = memo(ProfileFormInner);
