export const ENTITY_NAME = 'Client';
export const TEXTAREA_MIN_ROWS = 5;
export const FORM_NAME = 'Client form';

enum ClientFormField {
	Name = 'name',
	LegalEntityName = 'legalEntityName',
	Comments = 'comments',
	SaleManager = 'saleManager',
	RegisteredLocation = 'registeredLocation',
	Color = 'color',
}

export const CLIENT_FORM_FIELDS = {
	name: {
		label: 'Client name',
		name: ClientFormField.Name,
	},
	legalEntityName: {
		label: 'Legal entity name',
		name: ClientFormField.LegalEntityName,
	},
	comments: {
		label: 'Notes',
		name: ClientFormField.Comments,
	},
	saleManager: {
		label: 'Sale manager',
		name: ClientFormField.SaleManager,
		valueField: 'name',
	},
	registeredLocation: {
		label: 'Registered location',
		name: ClientFormField.RegisteredLocation,
		valueField: 'name',
	},
	color: {
		name: ClientFormField.Color,
	},
};
