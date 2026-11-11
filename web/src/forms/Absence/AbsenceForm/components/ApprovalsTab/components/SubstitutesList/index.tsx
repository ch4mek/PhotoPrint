import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { ArrowForwardIcon } from '@mms/mms-ui-library/icons/base/ArrowForwardIcon';
import { FormikErrors, FormikTouched, useFormikContext } from 'formik';
import { useCallback } from 'react';

import { RoleScopes } from '../../constants';
import { checkIsAnyUserRole } from '../../utils/checkIsAnyUserRole';
import { getScopeOptions } from '../../utils/getScopeOptions';
import { getUserOptions } from '../../utils/getUserOptions';
import { DynamicField } from '../DynamicField';

import { AbsenceFormValues } from '@/forms/Absence/AbsenceForm/types';
import { AcceptPolicesItem } from '@/types/absences';

import { SUBSTITUTES_MESSAGE } from './constants';
import {
	ArrowForwardWrapper,
	DynamicFieldWrapper,
	ScopeItem,
	ScopeItemTypography,
	SubstitutesItemWrapper,
	SubstitutesListWrapper,
	SubstitutesMessage,
	SubstitutesWrapper,
} from './styles';
import { SubstitutesListProps } from './types';

export function SubstitutesList({ users = [] }: SubstitutesListProps) {
	const { values, setFieldValue, errors, touched, setFieldTouched } =
		useFormikContext<AbsenceFormValues>();

	const { acceptPolicies } = values.approvals;
	const fieldsValues = acceptPolicies;
	const fieldErrors = errors?.approvals
		?.acceptPolicies as FormikErrors<AcceptPolicesItem>[];
	const fieldTouched = touched?.approvals
		?.acceptPolicies as FormikTouched<AcceptPolicesItem>[];

	const handleChangeValue = useCallback(
		(index: number) => (value: AcceptPolicesItem, changedField?: string) => {
			setFieldValue(
				`approvals.acceptPolicies.${index}`,
				{
					...value,
				},
				true
			);

			if (changedField) {
				setFieldTouched(
					`approvals.acceptPolicies.${index}.${changedField}`,
					true
				);

				if (changedField === RoleScopes.SubstituteScope) {
					setFieldTouched(
						`approvals.acceptPolicies.${index}.${RoleScopes.SubstituteUser}`,
						false
					);
				}
			} else {
				setFieldTouched(`approvals.acceptPolicies.${index}`, false);
			}
		},
		[setFieldValue]
	);

	return (
		<SubstitutesWrapper>
			<SubstitutesListWrapper>
				{acceptPolicies.map((value, index) => {
					const isDisableItem =
						value.scope && !checkIsAnyUserRole(value.scope.scope)
							? true
							: !!value.user?.name;

					const availableOptions = getScopeOptions(
						fieldsValues,
						true,
						value.scope?.scope
					);

					const usersOptions = getUserOptions(
						users,
						fieldsValues,
						true,
						value.user?.id
					);

					return (
						isDisableItem && (
							<SubstitutesItemWrapper key={index}>
								<ScopeItem>
									<Tooltip
										text={value.user?.name ?? value.scope?.name}
										useIfTextCut
									>
										<ScopeItemTypography variant="m-400">
											{value.user?.name ?? value.scope?.name}
										</ScopeItemTypography>
									</Tooltip>
								</ScopeItem>
								<ArrowForwardWrapper>
									<ArrowForwardIcon />
								</ArrowForwardWrapper>
								<DynamicFieldWrapper>
									<DynamicField
										value={value}
										roleOptionsValues={availableOptions}
										usersOptions={usersOptions}
										onChange={handleChangeValue(index)}
										error={fieldErrors && fieldErrors[index]}
										fieldTouched={fieldTouched && fieldTouched[index]}
										isShowUserField
										isSubstitute
									/>
								</DynamicFieldWrapper>
							</SubstitutesItemWrapper>
						)
					);
				})}
			</SubstitutesListWrapper>
			<SubstitutesMessage variant="s-400">
				{SUBSTITUTES_MESSAGE}
			</SubstitutesMessage>
		</SubstitutesWrapper>
	);
}
