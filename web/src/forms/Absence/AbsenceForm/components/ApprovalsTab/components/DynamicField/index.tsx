import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';

import { RoleScopes } from '../../constants';
import { checkIsAnyUserRole } from '../../utils/checkIsAnyUserRole';

import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { AcceptPolicesScope, AcceptPolicesUser } from '@/types/absences';

import { DynamicAutoselectData } from './constants';
import { DynamicFieldWrapper } from './styles';
import { DynamicFieldProps } from './types';

const { employee, employeeRole } = DynamicAutoselectData;

export function DynamicField({
	value,
	roleOptionsValues,
	usersOptions,
	onChange,
	isSubstitute = false,
	error,
	fieldTouched,
	isShowIcon = false,
	isShowUserField = false,
}: DynamicFieldProps) {
	const { scope, user, substituteScope, substituteUser } = value;

	const isUserRoleSelect =
		isShowUserField ||
		checkIsAnyUserRole(isSubstitute ? substituteScope?.scope : scope?.scope);
	const fieldKey = isSubstitute ? RoleScopes.SubstituteScope : RoleScopes.Scope;

	const handleScopeChange = (scopeValue: AcceptPolicesScope | null) => {
		const currentScope = isSubstitute ? substituteScope : scope;

		if (!scopeValue && currentScope) {
			onChange(
				{
					...value,
					...(isSubstitute
						? {
								substituteScope: null,
								substituteUser: null,
								substituteUserId: null,
						  }
						: {
								scope: null,
								substituteScope: null,
								user: null,
								userId: null,
						  }),
				},
				fieldKey
			);

			return;
		}

		if (!scopeValue) {
			return;
		}

		onChange(
			{
				...value,
				...(isSubstitute
					? {
							substituteScope: scopeValue,
							substituteUser: checkIsAnyUserRole(scopeValue.scope)
								? value.substituteUser
								: null,

							substituteUserId: checkIsAnyUserRole(scopeValue.scope)
								? value.substituteUser?.id
								: null,
					  }
					: {
							scope: scopeValue,
							substituteScope: null,
							substituteUserId: null,
							substituteUser: null,
							user: checkIsAnyUserRole(scopeValue.scope) ? value.user : null,
							userId: checkIsAnyUserRole(scopeValue.scope)
								? value.user?.id
								: null,
					  }),
			},
			fieldKey
		);
	};

	const changedField = isSubstitute
		? RoleScopes.SubstituteUser
		: RoleScopes.User;

	const handleEmployeeChange = (option: AcceptPolicesUser | null) => {
		onChange(
			{
				...value,
				...(isSubstitute
					? {
							substituteUser: option,
							substituteUserId: option?.id,
					  }
					: {
							user: option,
							userId: option?.id,
					  }),
			},
			changedField
		);
	};

	return (
		<DynamicFieldWrapper
			isShowIcon={isShowIcon}
			isShowUserField={isShowIcon || isUserRoleSelect}
		>
			<Autoselect
				{...employeeRole}
				value={isSubstitute ? substituteScope || null : scope}
				options={roleOptionsValues}
				onChange={handleScopeChange}
				error={
					isSubstitute
						? Boolean(fieldTouched?.substituteScope && error?.substituteScope)
						: Boolean(fieldTouched && error?.scope)
				}
				errorText={isSubstitute ? error?.substituteScope : error?.scope || ''}
				isRequired={!isSubstitute}
				disableClearable
			/>
			{checkIsAnyUserRole(
				isSubstitute ? substituteScope?.scope : scope?.scope
			) && (
				<Autoselect
					{...employee}
					value={isSubstitute ? substituteUser : user}
					options={usersOptions}
					onChange={handleEmployeeChange}
					disableClearable
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					error={
						isSubstitute
							? Boolean(fieldTouched?.substituteUser && error?.substituteUser)
							: Boolean(fieldTouched?.user && error?.user)
					}
					errorText={isSubstitute ? error?.substituteUser : error?.user || ''}
					isRequired
				/>
			)}
		</DynamicFieldWrapper>
	);
}
