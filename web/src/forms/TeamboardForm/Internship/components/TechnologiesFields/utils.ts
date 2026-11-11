import { FormikErrors } from 'formik';

import { AutoselectOption } from '@/types/common';
import {
	InternshipHardSkill,
	InternshipSeniority,
	InternshipTechnology,
	InternshipTechnologyPayload,
} from '@/types/internship';

import {
	createDefaultTechnologyItem,
	TECHNOLOGIES_FIELD_NAME,
} from './constants';
import { TechnologyFormItem, TechnologyHardSkillFormItem } from './types';

const getTechnologyFieldPrefix = (technologyIndex: number) =>
	`${TECHNOLOGIES_FIELD_NAME}.${technologyIndex}`;

const getHardSkillFieldPrefix = (technologyIndex: number, skillIndex: number) =>
	`${getTechnologyFieldPrefix(technologyIndex)}.hardSkills.${skillIndex}`;

export const getHardSkillsArrayPrefix = (technologyIndex: number) =>
	`${getTechnologyFieldPrefix(technologyIndex)}.hardSkills`;

export const getSpecializationName = (technologyIndex: number) =>
	`${getTechnologyFieldPrefix(technologyIndex)}.specialization`;

export const getSpecializationSeniorityName = (technologyIndex: number) =>
	`${getTechnologyFieldPrefix(technologyIndex)}.seniority`;

export const getHardSkillName = (technologyIndex: number, skillIndex: number) =>
	`${getHardSkillFieldPrefix(technologyIndex, skillIndex)}.hardSkill`;

export const getHardSkillSeniorityName = (
	technologyIndex: number,
	skillIndex: number
) => `${getHardSkillFieldPrefix(technologyIndex, skillIndex)}.seniority`;

export const getTechnologyError = <Item>(
	index: number,
	errors?: string | string[] | FormikErrors<Item>[]
) => {
	const item = Array.isArray(errors) ? errors[index] : errors;

	return typeof item === 'object' && item !== null
		? (item as FormikErrors<Item>)
		: undefined;
};

export const getVisibleFieldError = (
	error: unknown,
	isTouched: unknown,
	hasAttemptedSubmit: boolean
) => {
	if (typeof error !== 'string' || (!hasAttemptedSubmit && !isTouched)) {
		return undefined;
	}

	return error;
};

export const getAvailableSpecializationOptions = (
	allOptions: AutoselectOption[],
	technologies: TechnologyFormItem[],
	currentSpecializationId?: number
) => {
	const selectedIds = new Set(
		technologies
			.map(({ specialization }) => specialization?.id)
			.filter(
				(id): id is number => id != null && id !== currentSpecializationId
			)
	);

	return allOptions.filter(({ id }) => !selectedIds.has(id));
};

export const getAvailableHardSkillOptions = (
	allOptions: AutoselectOption[],
	hardSkills: TechnologyFormItem['hardSkills'],
	currentHardSkillId?: number
) => {
	const usedIds = new Set(
		hardSkills
			.map(({ hardSkill }) => hardSkill?.id)
			.filter((id): id is number => id != null && id !== currentHardSkillId)
	);

	return allOptions.filter(({ id }) => !usedIds.has(id));
};

export const appendSelectedOptions = (
	options: AutoselectOption[],
	selectedOptions: AutoselectOption[]
) =>
	selectedOptions.reduce((result, selected) => {
		if (result.some(({ id }) => id === selected.id)) {
			return result;
		}

		return [...result, selected];
	}, options);

const mapSeniorityOption = (
	seniorityLevelId?: string | null,
	seniorityLevelName?: string | null
): AutoselectOption<InternshipSeniority> | null => {
	if (!seniorityLevelId && !seniorityLevelName) {
		return null;
	}

	const matchedSeniority = Object.values(InternshipSeniority).find(
		(seniority) =>
			seniority.toLowerCase() === seniorityLevelId?.toLowerCase() ||
			seniority.toLowerCase() === seniorityLevelName?.toLowerCase()
	);

	if (matchedSeniority) {
		return { id: matchedSeniority, name: matchedSeniority };
	}

	if (seniorityLevelId && seniorityLevelName) {
		return {
			id: seniorityLevelId as InternshipSeniority,
			name: seniorityLevelName,
		};
	}

	return null;
};

const mapHardSkillToFormItem = ({
	id,
	name,
	seniorityLevelId,
	seniorityLevelName,
}: InternshipHardSkill): TechnologyHardSkillFormItem => ({
	rowId: crypto.randomUUID(),
	hardSkill: { id, name },
	seniority: mapSeniorityOption(seniorityLevelId, seniorityLevelName),
});

const mapTechnologyToFormItem = ({
	id,
	specializationTemplateId,
	specializationTemplateName,
	seniorityLevelId,
	seniorityLevelName,
	hardSkills,
}: InternshipTechnology): TechnologyFormItem => ({
	rowId: crypto.randomUUID(),
	id,
	specialization: {
		id: specializationTemplateId,
		name: specializationTemplateName,
	},
	seniority: mapSeniorityOption(seniorityLevelId, seniorityLevelName),
	hardSkills: hardSkills.map(mapHardSkillToFormItem),
});

export const mapTechnologiesToFormItems = (
	technologies: InternshipTechnology[]
) => {
	if (technologies.length === 0) {
		return [createDefaultTechnologyItem()];
	}

	return technologies.map(mapTechnologyToFormItem);
};

const mapHardSkillFormItemToPayload = (
	{ hardSkill, seniority }: TechnologyHardSkillFormItem,
	fallbackSeniority: AutoselectOption<InternshipSeniority>
): InternshipHardSkill[] => {
	if (hardSkill == null) {
		return [];
	}

	const resolvedSeniority = seniority ?? fallbackSeniority;

	return [
		{
			id: hardSkill.id,
			name: hardSkill.name,
			seniorityLevelId: resolvedSeniority.id,
			seniorityLevelName: resolvedSeniority.name,
		},
	];
};

export const mapFormItemsToTechnologies = (
	technologies: TechnologyFormItem[]
): InternshipTechnologyPayload[] =>
	technologies.flatMap(({ id, specialization, seniority, hardSkills }) => {
		if (specialization == null || seniority == null) {
			return [];
		}

		return [
			{
				id,
				specializationTemplateId: specialization.id,
				specializationTemplateName: specialization.name,
				seniorityLevelId: seniority.id,
				seniorityLevelName: seniority.name,
				hardSkills: hardSkills.flatMap((hardSkillItem) =>
					mapHardSkillFormItemToPayload(hardSkillItem, seniority)
				),
			},
		];
	});
