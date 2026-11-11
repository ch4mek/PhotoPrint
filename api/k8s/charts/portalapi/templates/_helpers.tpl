{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "this.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "this.labels" -}}
helm.sh/chart: {{ include "this.chart" . }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
{{ include "this.selectorLabels" . }}
namespace: {{ .Release.Namespace }}
environment: {{ .Values.env }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "this.selectorLabels" -}}
app.kubernetes.io/name: {{ .Values.app }}
{{- end }}
