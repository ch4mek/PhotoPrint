import { roleOptions } from '../components/ApprovelsFieldsGroup/constants';

export const mapScope = (scope?: string | null) =>
	scope ? roleOptions.find((option) => option.scope === scope) ?? null : null;
