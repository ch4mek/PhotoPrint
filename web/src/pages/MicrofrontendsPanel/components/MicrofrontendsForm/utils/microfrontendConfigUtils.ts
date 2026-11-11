import { MicrofrontendsFormValues } from '../types';

import { UserClaims } from '@/types/claims';
import { MicroFrontendConfig } from '@/types/microfrontends';

const convertFormValuesToMicrofrontend = (
	formData: MicrofrontendsFormValues
): MicroFrontendConfig => {
	const claimsArray = formData.claims
		? formData.claims.map((claim) =>
				typeof claim === 'string' ? claim : (claim as UserClaims)
		  )
		: [];

	const remoteEntriesString =
		typeof formData.remoteEntries === 'string' ? formData.remoteEntries : '';

	return {
		name: formData.name,
		scope: formData.scope,
		claims: claimsArray as UserClaims[],
		modules: formData.modules,
		routes: formData.routes || [],
		version: formData.version,
		remoteEntries: remoteEntriesString,
	};
};

export const addMicrofrontendToConfig = (
	existingMicrofrontends: MicroFrontendConfig[],
	newMicrofrontend: MicrofrontendsFormValues
): MicroFrontendConfig[] => {
	const converted = convertFormValuesToMicrofrontend(newMicrofrontend);

	return [...existingMicrofrontends, converted];
};

export const updateMicrofrontendInConfig = (
	existingMicrofrontends: MicroFrontendConfig[],
	microfrontendName: string,
	updatedData: MicrofrontendsFormValues
): MicroFrontendConfig[] => {
	const converted = convertFormValuesToMicrofrontend(updatedData);

	return existingMicrofrontends.map((mfe) =>
		mfe.name === microfrontendName ? converted : mfe
	);
};

export const removeMicrofrontendFromConfig = (
	existingMicrofrontends: MicroFrontendConfig[],
	microfrontendName: string
): MicroFrontendConfig[] =>
	existingMicrofrontends.filter((mfe) => mfe.name !== microfrontendName);
