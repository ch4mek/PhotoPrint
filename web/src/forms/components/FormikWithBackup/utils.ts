import { hashString } from '@/helpers/hashString';

import { FORM_BACKUPS_STORAGE_KEY } from './constants';

const serializeData = (data: unknown) => JSON.stringify(data);
const deserializeData = (data: string) => JSON.parse(data);

export const isEqual = (value: any, other: any): boolean => {
	if (
		(value === null && other === undefined) ||
		(value === undefined && other === null)
	) {
		return true;
	}

	if (Array.isArray(value) && Array.isArray(other)) {
		if (value.length !== other.length) {
			return false;
		}

		return value.every((item, i) => isEqual(item, other[i]));
	}

	if (
		typeof value === 'object' &&
		typeof other === 'object' &&
		value &&
		other
	) {
		if (
			(Array.isArray(value) && !Array.isArray(other)) ||
			(!Array.isArray(value) && Array.isArray(other))
		) {
			return false;
		}

		return Object.keys(value).every((key) => isEqual(value[key], other[key]));
	}

	return value === other;
};

type GetFormKeyArgs = {
	userId: number;
	name: string;
	id?: string | number | null;
};

export const getFormKey = ({
	userId,
	name,
	id = '',
}: GetFormKeyArgs): string => {
	const canonical = [userId, name.trim().toLowerCase(), id].join('|');

	return hashString(canonical);
};

const getAllFormBackups = () => {
	const serializedBackups = sessionStorage.getItem(FORM_BACKUPS_STORAGE_KEY);

	if (!serializedBackups) {
		return null;
	}

	return deserializeData(serializedBackups);
};

export const getDataFromStorage = (formKey: string) => {
	const allFormBackups = getAllFormBackups();

	if (!allFormBackups || !(formKey in allFormBackups)) {
		return null;
	}

	return allFormBackups[formKey];
};

export const putDataInStorage = (data: unknown, formKey: string) => {
	const allFormBackups = getAllFormBackups();

	if (allFormBackups) {
		allFormBackups[formKey] = data;
		sessionStorage.setItem(
			FORM_BACKUPS_STORAGE_KEY,
			serializeData(allFormBackups)
		);
	} else {
		sessionStorage.setItem(
			FORM_BACKUPS_STORAGE_KEY,
			serializeData({ [formKey]: data })
		);
	}
};

export const removeDataFromStorage = (formKey: string) => {
	const allFormBackups = getAllFormBackups();

	if (allFormBackups && formKey in allFormBackups) {
		delete allFormBackups[formKey];
		sessionStorage.setItem(
			FORM_BACKUPS_STORAGE_KEY,
			serializeData(allFormBackups)
		);
	}
};
