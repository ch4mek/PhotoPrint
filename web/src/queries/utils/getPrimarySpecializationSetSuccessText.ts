const SUCCESS_SET_PRIMARY_SPECIALIZATION_TEXT =
	'specialization has been successfully set as the primary';

export function getPrimarySpecializationSetSuccessText(
	specializationName: string
) {
	return `${specializationName} ${SUCCESS_SET_PRIMARY_SPECIALIZATION_TEXT}`;
}
