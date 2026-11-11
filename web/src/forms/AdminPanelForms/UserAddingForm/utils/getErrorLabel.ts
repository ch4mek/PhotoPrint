import { ADMIN_PANEL_USER_FORM_FIELDS } from '../constants';

export const getErrorLabel = (
	field: keyof typeof ADMIN_PANEL_USER_FORM_FIELDS
) => ADMIN_PANEL_USER_FORM_FIELDS[field].label.toLowerCase();
