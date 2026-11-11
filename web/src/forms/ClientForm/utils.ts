import { Theme } from '@emotion/react';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { getFullName } from '@/helpers/utils';
import { ClientResponse } from '@/types/clients';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import { LookupSaleManagersResponse } from '@/types/interview';

import { ClientFormValues } from './types';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: true,
		windowHeight,
	}),
});

export const mapSaleManagersToOptions = (
	users?: LookupSaleManagersResponse[]
): SimpleConfigurableConfigurationItem<number>[] =>
	(users || []).map(({ id, firstName, lastName, firstNameRu, lastNameRu }) => ({
		id,
		name: getFullName(firstName, lastName, firstNameRu, lastNameRu),
	}));

export const mapClientToFormValues = ({
	name,
	legalEntityName,
	comments,
	registeredLocation,
	color,
	salesManagerId,
	salesManagerFullName,
}: ClientResponse): ClientFormValues => ({
	name,
	legalEntityName: legalEntityName ?? '',
	comments,
	registeredLocation,
	color,
	saleManager:
		salesManagerId && salesManagerFullName
			? {
					id: salesManagerId,
					name: salesManagerFullName,
			  }
			: null,
});
