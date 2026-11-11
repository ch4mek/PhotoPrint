import * as yup from 'yup';

import { textType4Schema } from '@/forms/schemas';

import {
	mfeConfigValidationErrors,
	MIN_FORM_VALUE_LENGTH,
	MIN_MODULE_VALUE_LENGTH,
	MODULE_PREFIX,
	ROUTE_PREFIX,
} from './constants';

const routesValidation = yup
	.array()
	.of(
		yup
			.string()
			.optional()
			.test(
				'starts-with-slash',
				mfeConfigValidationErrors.routes.startsWithSlash,
				(value) => (value ? value.startsWith(ROUTE_PREFIX) : false)
			)
			.min(MIN_FORM_VALUE_LENGTH, mfeConfigValidationErrors.routes.minLength)
	)
	.nullable()
	.optional();

const modulesValidation = yup
	.array()
	.of(
		yup
			.string()
			.optional()
			.test(
				'starts-with-dot-slash',
				mfeConfigValidationErrors.modules.startsWithDotSlash,
				(value) => (value ? value.startsWith(MODULE_PREFIX) : false)
			)
			.min(MIN_MODULE_VALUE_LENGTH, mfeConfigValidationErrors.modules.minLength)
	)
	.nullable()
	.optional();

const claimsValidation = yup.array().nullable().optional();

const remoteEntriesValidation = yup
	.string()
	.required(mfeConfigValidationErrors.remoteEnties.required)
	.test(
		'is-valid-url-or-localhost',
		mfeConfigValidationErrors.remoteEnties.invalidUrl,
		async (value) => {
			if (!value) {
				return false;
			}

			if (value.includes('localhost')) {
				try {
					const url = new URL(value);

					return ['http:', 'https:'].includes(url.protocol);
				} catch {
					return false;
				}
			}

			try {
				await yup.string().url().validate(value);

				return true;
			} catch {
				return false;
			}
		}
	);

export const microfrontendsValidationSchema = yup.object().shape({
	name: textType4Schema('Name'),
	scope: textType4Schema('Scope', false),
	module: modulesValidation,
	remoteEntries: remoteEntriesValidation,
	routes: routesValidation,
	claims: claimsValidation,
	version: textType4Schema('Version', false),
});
