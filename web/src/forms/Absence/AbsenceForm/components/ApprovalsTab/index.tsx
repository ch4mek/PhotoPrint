import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { spaces } from '@mms/mms-ui-library/theme';
import { Form } from 'formik';
import { ForwardedRef, forwardRef } from 'react';

import { CheckboxLabel, HiddenSubmitButtonWrapper } from '../DetailsTab/styles';
import { FieldWithDescription } from '../FieldWithDescription';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useApproversUsersLookup } from '@/queries/inquiries/useApproversUsersLookup';

import { ApprovelsFieldsGroup } from './components/ApprovelsFieldsGroup';
import { SubstitutesList } from './components/SubstitutesList';
import { absenceApprovalsFieldData } from './constants';
import { checkboxWrapperStyles, FormWrapper } from './styles';
import { ApprovalsFormValuesProps, ApprovalsTabProps } from './types';

function ApprovalsTabInner(
	{ values, setFieldValue }: ApprovalsTabProps,
	ref: ForwardedRef<HTMLButtonElement>
) {
	const toast = useToast();

	const { isSubstitutesToApproveAllowed } = values;

	const { data: users } = useApproversUsersLookup({
		onError: createOnErrorHandler(toast),
	});

	const handleCheckBoxClick = (field: keyof ApprovalsFormValuesProps) => () => {
		setFieldValue(`approvals.${field}`, !values[field]);
	};

	return (
		<Form>
			<FormWrapper>
				<ApprovelsFieldsGroup {...{ users }} name="acceptPolicies" />
				<Checkbox
					{...absenceApprovalsFieldData.isSubstitutesToApproveAllowed}
					key={absenceApprovalsFieldData.isSubstitutesToApproveAllowed.key}
					checked={isSubstitutesToApproveAllowed}
					onClick={handleCheckBoxClick('isSubstitutesToApproveAllowed')}
					wrapperStyles={checkboxWrapperStyles}
					label={
						<FieldWithDescription
							{...{
								description:
									absenceApprovalsFieldData.isSubstitutesToApproveAllowed
										.description,
							}}
							gapSize={spaces.xs}
						>
							<CheckboxLabel variant="m-400">
								{absenceApprovalsFieldData.isSubstitutesToApproveAllowed.label}
							</CheckboxLabel>
						</FieldWithDescription>
					}
				/>
				{isSubstitutesToApproveAllowed && <SubstitutesList {...{ users }} />}
				<HiddenSubmitButtonWrapper type="submit" ref={ref} />
			</FormWrapper>
		</Form>
	);
}

export const ApprovalsTab = forwardRef(ApprovalsTabInner);
