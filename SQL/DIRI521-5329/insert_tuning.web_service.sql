use BillingNT;

insert into Tuning.web_service(code, extra, url)
values ('jira_api', '<ServiceSettings>
  <service_url>https://test.plan.corp.itmh.ru</service_url>
  <login>service_atlas_assist</login>
  <password>HDgbf67sgwk</password>
</ServiceSettings>', '')

insert into Tuning.web_service(code, extra, url)
values ('ldap', '<ServiceSettings>
  <service_url>ldap.corp.itmh.ru</service_url>
  <login>service_sas</login>
  <password>DH6dl38Ss2</password>
  <port>389</port>
</ServiceSettings>', '')
