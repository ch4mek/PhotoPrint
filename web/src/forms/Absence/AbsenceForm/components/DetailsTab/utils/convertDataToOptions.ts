import {
	PersistentConfigurationItem,
	SimpleConfigurableConfigurationItem,
} from '@/types/configuration';

const getArrayOption = (
	id: string,
	optionsObject: Record<string, string>
): SimpleConfigurableConfigurationItem => ({
	id,
	name: optionsObject[id],
});

export const convertDataToOptions = (
	optionsObject: Record<string, string>,
	data?: Array<PersistentConfigurationItem>
) => {
	if (!data) {
		return [];
	}

	return data.map(({ id }) => getArrayOption(id, optionsObject));
};
