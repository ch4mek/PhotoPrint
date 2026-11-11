import {
	mfeConfigValidationErrors,
	MIN_FORM_VALUE_LENGTH,
	MIN_MODULE_VALUE_LENGTH,
	MODULE_PREFIX,
	ROUTE_PREFIX,
} from '@/pages/MicrofrontendsPanel/components/MicrofrontendsForm/constants';

export const validateRoute = (value: string): string | null => {
	if (!value.startsWith(ROUTE_PREFIX)) {
		return mfeConfigValidationErrors.routes.startsWithSlash;
	}

	if (value.length < MIN_FORM_VALUE_LENGTH) {
		return mfeConfigValidationErrors.routes.minLength;
	}

	return null;
};

export const validateModule = (value: string): string | null => {
	if (!value.startsWith(MODULE_PREFIX)) {
		return mfeConfigValidationErrors.modules.startsWithDotSlash;
	}

	if (value.length < MIN_MODULE_VALUE_LENGTH) {
		return mfeConfigValidationErrors.modules.minLength;
	}

	return null;
};
