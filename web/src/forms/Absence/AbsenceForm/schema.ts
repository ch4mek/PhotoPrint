import * as yup from 'yup';

import { absencesType } from '@/constants/absence';
import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { SHORT_SAME_ABSENCE_NAME_ALREADY_EXISTS } from '@/constants/notifications';
import { longTextType1Schema, textType1Schema } from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';
import { AcceptPolicesScope } from '@/types/absences';

import { checkIsAnyUserRole } from './components/ApprovalsTab/utils/checkIsAnyUserRole';
import { absenceDetailsFieldData } from './components/DetailsTab/constants';

const { name, comment } = absenceDetailsFieldData;

const whenVacationOrSickDay = (schema: any) =>
	schema.when('type', {
		is: (type: any) =>
			type === absencesType.Vacation || type === absencesType.SickDay,
		then: (finalSchema: any) => finalSchema.required(FIELD_IS_REQUIRED),
		otherwise: (finalSchema: any) => finalSchema.notRequired(),
	});

const createDetailsSchema = (
	checkIsNameExist: (name: string, id?: string | number) => boolean,
	currentId?: string | number
) =>
	yup.object().shape({
		name: textType1Schema(name.label.toLowerCase()).test(
			'unique-name',
			SHORT_SAME_ABSENCE_NAME_ALREADY_EXISTS,
			(value) => {
				if (!value) {
					return true;
				}

				return !checkIsNameExist(value, currentId);
			}
		),
		type: ddlType1Schema(),
		trackingUnitType: ddlType1Schema(),
		activityType: ddlType1Schema(),
		comment: longTextType1Schema(comment.label.toLowerCase()),
		accrualType: whenVacationOrSickDay(ddlType1Schema().nullable()),
		transferType: whenVacationOrSickDay(ddlType1Schema().nullable()),
	});

const approvalsSchema = yup.object().shape({
	isSubstitutesToApproveAllowed: yup.boolean(),
	acceptPolicies: yup.lazy((value, options) => {
		const isSubstitutesToApproveAllowed =
			options.parent?.isSubstitutesToApproveAllowed;

		return yup
			.array()
			.of(
				yup
					.object()
					.shape({
						scope: ddlType1Schema().nullable(),
						substituteScope: ddlType1Schema().nullable().optional(),
						substituteUser: isSubstitutesToApproveAllowed
							? ddlType1Schema()
									.nullable()
									.when('substituteScope', {
										is: (substituteScope: AcceptPolicesScope) =>
											!checkIsAnyUserRole(substituteScope),
										then: (schema) => schema.optional(),
									})
							: ddlType1Schema().nullable().optional(),

						user: ddlType1Schema()
							.nullable()
							.when('scope', {
								is: (scope: AcceptPolicesScope) => !checkIsAnyUserRole(scope),
								then: (schema) => schema.optional(),
							}),
					})
					.nullable()
			)
			.nullable();
	}),
});

export const createAbsenceValidationSchema = (
	checkIsNameExist: (name: string, id?: string | number) => boolean,
	currentId?: string | number
) =>
	yup.object().shape({
		details: createDetailsSchema(checkIsNameExist, currentId),
		approvals: approvalsSchema,
	});
